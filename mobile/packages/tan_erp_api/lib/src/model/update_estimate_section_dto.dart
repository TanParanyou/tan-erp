//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/update_estimate_work_item_dto.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'update_estimate_section_dto.g.dart';

/// UpdateEstimateSectionDto
///
/// Properties:
/// * [id] 
/// * [code] 
/// * [nameTh] 
/// * [nameEn] 
/// * [sortOrder] 
/// * [workItems] 
@BuiltValue()
abstract class UpdateEstimateSectionDto implements Built<UpdateEstimateSectionDto, UpdateEstimateSectionDtoBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'code')
  String? get code;

  @BuiltValueField(wireName: r'nameTh')
  String? get nameTh;

  @BuiltValueField(wireName: r'nameEn')
  String? get nameEn;

  @BuiltValueField(wireName: r'sortOrder')
  int? get sortOrder;

  @BuiltValueField(wireName: r'workItems')
  BuiltList<UpdateEstimateWorkItemDto>? get workItems;

  UpdateEstimateSectionDto._();

  factory UpdateEstimateSectionDto([void updates(UpdateEstimateSectionDtoBuilder b)]) = _$UpdateEstimateSectionDto;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(UpdateEstimateSectionDtoBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<UpdateEstimateSectionDto> get serializer => _$UpdateEstimateSectionDtoSerializer();
}

class _$UpdateEstimateSectionDtoSerializer implements PrimitiveSerializer<UpdateEstimateSectionDto> {
  @override
  final Iterable<Type> types = const [UpdateEstimateSectionDto, _$UpdateEstimateSectionDto];

  @override
  final String wireName = r'UpdateEstimateSectionDto';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    UpdateEstimateSectionDto object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.code != null) {
      yield r'code';
      yield serializers.serialize(
        object.code,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.nameTh != null) {
      yield r'nameTh';
      yield serializers.serialize(
        object.nameTh,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.nameEn != null) {
      yield r'nameEn';
      yield serializers.serialize(
        object.nameEn,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.sortOrder != null) {
      yield r'sortOrder';
      yield serializers.serialize(
        object.sortOrder,
        specifiedType: const FullType(int),
      );
    }
    if (object.workItems != null) {
      yield r'workItems';
      yield serializers.serialize(
        object.workItems,
        specifiedType: const FullType.nullable(BuiltList, [FullType(UpdateEstimateWorkItemDto)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    UpdateEstimateSectionDto object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required UpdateEstimateSectionDtoBuilder result,
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
        case r'code':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.code = valueDes;
          break;
        case r'nameTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nameTh = valueDes;
          break;
        case r'nameEn':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.nameEn = valueDes;
          break;
        case r'sortOrder':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sortOrder = valueDes;
          break;
        case r'workItems':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(UpdateEstimateWorkItemDto)]),
          ) as BuiltList<UpdateEstimateWorkItemDto>?;
          if (valueDes == null) continue;
          result.workItems.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  UpdateEstimateSectionDto deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = UpdateEstimateSectionDtoBuilder();
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


