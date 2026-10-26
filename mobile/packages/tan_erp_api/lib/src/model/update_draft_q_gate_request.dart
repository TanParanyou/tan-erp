//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_draft_q_gate_request.g.dart';

/// UpdateDraftQGateRequest
///
/// Properties:
/// * [scopeSummary] 
/// * [workTypes] 
/// * [nextActionAtUtc] 
/// * [nextActionNote] 
@BuiltValue()
abstract class UpdateDraftQGateRequest implements Built<UpdateDraftQGateRequest, UpdateDraftQGateRequestBuilder> {
  @BuiltValueField(wireName: r'scopeSummary')
  String? get scopeSummary;

  @BuiltValueField(wireName: r'workTypes')
  BuiltList<String>? get workTypes;

  @BuiltValueField(wireName: r'nextActionAtUtc')
  DateTime? get nextActionAtUtc;

  @BuiltValueField(wireName: r'nextActionNote')
  String? get nextActionNote;

  UpdateDraftQGateRequest._();

  factory UpdateDraftQGateRequest([void updates(UpdateDraftQGateRequestBuilder b)]) = _$UpdateDraftQGateRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateDraftQGateRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateDraftQGateRequest> get serializer => _$UpdateDraftQGateRequestSerializer();
}

class _$UpdateDraftQGateRequestSerializer implements PrimitiveSerializer<UpdateDraftQGateRequest> {
  @override
  final Iterable<Type> types = const [UpdateDraftQGateRequest, _$UpdateDraftQGateRequest];

  @override
  final String wireName = r'UpdateDraftQGateRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateDraftQGateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.scopeSummary != null) {
      yield r'scopeSummary';
      yield serializers.serialize(
        object.scopeSummary,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.workTypes != null) {
      yield r'workTypes';
      yield serializers.serialize(
        object.workTypes,
        specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
      );
    }
    if (object.nextActionAtUtc != null) {
      yield r'nextActionAtUtc';
      yield serializers.serialize(
        object.nextActionAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
    if (object.nextActionNote != null) {
      yield r'nextActionNote';
      yield serializers.serialize(
        object.nextActionNote,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateDraftQGateRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateDraftQGateRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'scopeSummary':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.scopeSummary = valueDes;
          break;
        case r'workTypes':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(String)]),
          ) as BuiltList<String>?;
          if (valueDes == null) continue;
          result.workTypes.replace(valueDes);
          break;
        case r'nextActionAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.nextActionAtUtc = valueDes;
          break;
        case r'nextActionNote':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nextActionNote = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateDraftQGateRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateDraftQGateRequestBuilder();
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


