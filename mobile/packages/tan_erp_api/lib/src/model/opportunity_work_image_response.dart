//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/work_image_user_summary_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'opportunity_work_image_response.g.dart';

/// OpportunityWorkImageResponse
///
/// Properties:
/// * [id] 
/// * [fileId] 
/// * [stageAtAttach] 
/// * [caption] 
/// * [displayOrder] 
/// * [createdAtUtc] 
/// * [createdBy] 
@BuiltValue()
abstract class OpportunityWorkImageResponse implements Built<OpportunityWorkImageResponse, OpportunityWorkImageResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'fileId')
  String? get fileId;

  @BuiltValueField(wireName: r'stageAtAttach')
  String? get stageAtAttach;

  @BuiltValueField(wireName: r'caption')
  String? get caption;

  @BuiltValueField(wireName: r'displayOrder')
  int? get displayOrder;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'createdBy')
  WorkImageUserSummaryResponse? get createdBy;

  OpportunityWorkImageResponse._();

  factory OpportunityWorkImageResponse([void updates(OpportunityWorkImageResponseBuilder b)]) = _$OpportunityWorkImageResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(OpportunityWorkImageResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<OpportunityWorkImageResponse> get serializer => _$OpportunityWorkImageResponseSerializer();
}

class _$OpportunityWorkImageResponseSerializer implements PrimitiveSerializer<OpportunityWorkImageResponse> {
  @override
  final Iterable<Type> types = const [OpportunityWorkImageResponse, _$OpportunityWorkImageResponse];

  @override
  final String wireName = r'OpportunityWorkImageResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    OpportunityWorkImageResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.fileId != null) {
      yield r'fileId';
      yield serializers.serialize(
        object.fileId,
        specifiedType: const FullType(String),
      );
    }
    if (object.stageAtAttach != null) {
      yield r'stageAtAttach';
      yield serializers.serialize(
        object.stageAtAttach,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.caption != null) {
      yield r'caption';
      yield serializers.serialize(
        object.caption,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.displayOrder != null) {
      yield r'displayOrder';
      yield serializers.serialize(
        object.displayOrder,
        specifiedType: const FullType(int),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(WorkImageUserSummaryResponse),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    OpportunityWorkImageResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required OpportunityWorkImageResponseBuilder result,
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
        case r'fileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.fileId = valueDes;
          break;
        case r'stageAtAttach':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.stageAtAttach = valueDes;
          break;
        case r'caption':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.caption = valueDes;
          break;
        case r'displayOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.displayOrder = valueDes;
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(WorkImageUserSummaryResponse),
          ) as WorkImageUserSummaryResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  OpportunityWorkImageResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = OpportunityWorkImageResponseBuilder();
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


