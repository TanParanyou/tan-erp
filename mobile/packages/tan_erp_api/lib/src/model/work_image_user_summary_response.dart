//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'work_image_user_summary_response.g.dart';

/// WorkImageUserSummaryResponse
///
/// Properties:
/// * [id] 
/// * [displayName] 
@BuiltValue()
abstract class WorkImageUserSummaryResponse implements Built<WorkImageUserSummaryResponse, WorkImageUserSummaryResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'displayName')
  String? get displayName;

  WorkImageUserSummaryResponse._();

  factory WorkImageUserSummaryResponse([void updates(WorkImageUserSummaryResponseBuilder b)]) = _$WorkImageUserSummaryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(WorkImageUserSummaryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<WorkImageUserSummaryResponse> get serializer => _$WorkImageUserSummaryResponseSerializer();
}

class _$WorkImageUserSummaryResponseSerializer implements PrimitiveSerializer<WorkImageUserSummaryResponse> {
  @override
  final Iterable<Type> types = const [WorkImageUserSummaryResponse, _$WorkImageUserSummaryResponse];

  @override
  final String wireName = r'WorkImageUserSummaryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    WorkImageUserSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.displayName != null) {
      yield r'displayName';
      yield serializers.serialize(
        object.displayName,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    WorkImageUserSummaryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required WorkImageUserSummaryResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'displayName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.displayName = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  WorkImageUserSummaryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = WorkImageUserSummaryResponseBuilder();
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


