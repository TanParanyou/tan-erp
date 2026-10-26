//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'branch_blocker_response.g.dart';

/// BranchBlockerResponse
///
/// Properties:
/// * [type] 
/// * [count] 
@BuiltValue()
abstract class BranchBlockerResponse implements Built<BranchBlockerResponse, BranchBlockerResponseBuilder> {
  @BuiltValueField(wireName: r'type')
  String? get type;

  @BuiltValueField(wireName: r'count')
  int? get count;

  BranchBlockerResponse._();

  factory BranchBlockerResponse([void updates(BranchBlockerResponseBuilder b)]) = _$BranchBlockerResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BranchBlockerResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BranchBlockerResponse> get serializer => _$BranchBlockerResponseSerializer();
}

class _$BranchBlockerResponseSerializer implements PrimitiveSerializer<BranchBlockerResponse> {
  @override
  final Iterable<Type> types = const [BranchBlockerResponse, _$BranchBlockerResponse];

  @override
  final String wireName = r'BranchBlockerResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BranchBlockerResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.type != null) {
      yield r'type';
      yield serializers.serialize(
        object.type,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.count != null) {
      yield r'count';
      yield serializers.serialize(
        object.count,
        specifiedType: const FullType(int),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BranchBlockerResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BranchBlockerResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'type':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.type = valueDes;
          break;
        case r'count':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.count = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BranchBlockerResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BranchBlockerResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


