//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'set_branch_availability_request.g.dart';

/// SetBranchAvailabilityRequest
///
/// Properties:
/// * [mode] 
/// * [branchIds] 
@BuiltValue()
abstract class SetBranchAvailabilityRequest implements Built<SetBranchAvailabilityRequest, SetBranchAvailabilityRequestBuilder> {
  @BuiltValueField(wireName: r'mode')
  String get mode;

  @BuiltValueField(wireName: r'branchIds')
  BuiltList<String>? get branchIds;

  SetBranchAvailabilityRequest._();

  factory SetBranchAvailabilityRequest([void updates(SetBranchAvailabilityRequestBuilder b)]) = _$SetBranchAvailabilityRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(SetBranchAvailabilityRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<SetBranchAvailabilityRequest> get serializer => _$SetBranchAvailabilityRequestSerializer();
}

class _$SetBranchAvailabilityRequestSerializer implements PrimitiveSerializer<SetBranchAvailabilityRequest> {
  @override
  final Iterable<Type> types = const [SetBranchAvailabilityRequest, _$SetBranchAvailabilityRequest];

  @override
  final String wireName = r'SetBranchAvailabilityRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    SetBranchAvailabilityRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'mode';
    yield serializers.serialize(
      object.mode,
      specifiedType: const FullType(String),
    );
    if (object.branchIds != null) {
      yield r'branchIds';
      yield serializers.serialize(
        object.branchIds,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    SetBranchAvailabilityRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required SetBranchAvailabilityRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'mode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.mode = valueDes;
          break;
        case r'branchIds':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.branchIds.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  SetBranchAvailabilityRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = SetBranchAvailabilityRequestBuilder();
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


